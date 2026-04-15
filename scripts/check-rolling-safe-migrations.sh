#!/usr/bin/env bash
set -euo pipefail

base_ref="${1:-}"

if [[ -z "$base_ref" ]]; then
  if [[ -n "${GITHUB_BASE_REF:-}" ]]; then
    base_ref="origin/${GITHUB_BASE_REF}"
  else
    base_ref="HEAD~1"
  fi
fi

if ! git rev-parse --verify "$base_ref" >/dev/null 2>&1; then
  echo "Base ref '$base_ref' is not available; fetching full history may be required." >&2
  exit 1
fi

mapfile -t changed_migrations < <(
  git diff --name-only --diff-filter=ACMRT "$base_ref"...HEAD |
    grep -E '^src/Infrastructure/Migrations/[0-9].*\.cs$' |
    grep -Ev '\.Designer\.cs$' || true
)

if [[ "${#changed_migrations[@]}" -eq 0 ]]; then
  echo "No changed EF migration files to check."
  exit 0
fi

dangerous_pattern='migrationBuilder\.(Drop(Table|Column|Index|PrimaryKey|ForeignKey)|Rename(Table|Column|Index)|AlterColumn)|ALTER[[:space:]]+TABLE|DROP[[:space:]]+(TABLE|COLUMN|INDEX|CONSTRAINT)|RENAME[[:space:]]+(TABLE|COLUMN|INDEX)|TRUNCATE[[:space:]]+TABLE|DELETE[[:space:]]+FROM'
failed=0

for migration in "${changed_migrations[@]}"; do
  if grep -q 'rolling-upgrade-reviewed' "$migration"; then
    echo "Skipping reviewed rolling-upgrade migration: $migration"
    continue
  fi

  findings="$(
    awk '
      /protected override void Up\(MigrationBuilder migrationBuilder\)/ { in_up = 1 }
      /protected override void Down\(MigrationBuilder migrationBuilder\)/ { in_up = 0 }
      in_up { print FNR ":" $0 }
    ' "$migration" | grep -Ei "$dangerous_pattern" || true
  )"

  if [[ -n "$findings" ]]; then
    failed=1
    echo "::error file=$migration::Migration contains operations that can break rolling upgrades."
    echo "$findings"
    echo
  fi
done

if [[ "$failed" -ne 0 ]]; then
  cat <<'EOF'
Rolling upgrades require expand/contract migrations:
- First deploy additive, backward-compatible schema changes.
- Deploy application code that works with both old and new schema.
- Backfill data while both versions remain compatible.
- Remove or rename old schema only in a later deployment.

If this migration has been reviewed and intentionally belongs to a contraction
deployment, add this exact marker in the migration file:
// rolling-upgrade-reviewed
EOF
  exit 1
fi

echo "Changed EF migrations look compatible with rolling upgrades."
