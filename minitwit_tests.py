# -*- coding: utf-8 -*-
"""
    MiniTwit Tests
    ~~~~~~~~~~~~~~

    Tests the MiniTwit application.

    :copyright: (c) 2010 by Armin Ronacher.
    :license: BSD, see LICENSE for more details.
"""
import minitwit
import unittest
import tempfile


class MiniTwitTestCase(unittest.TestCase):

    def setUp(self):
        """Before each test, set up a blank database"""
        self.db = tempfile.NamedTemporaryFile()
        self.app = minitwit.app.test_client()
        minitwit.DATABASE = self.db.name
        minitwit.init_db()

    # helper functions

    def register(self, username, password, password2=None, email=None):
        """Helper function to register a user"""
        if password2 is None:
            password2 = password
        if email is None:
            email = username + '@example.com'
        return self.app.post('/register', data={
            'username':     username,
            'password':     password,
            'password2':    password2,
            'email':        email,
        }, follow_redirects=True)

    def login(self, username, password):
        """Helper function to login"""
        return self.app.post('/login', data={
            'username': username,
            'password': password
        }, follow_redirects=True)

    def register_and_login(self, username, password):
        """Registers and logs in in one go"""
        self.register(username, password)
        return self.login(username, password)

    def logout(self):
        """Helper function to logout"""
        return self.app.get('/logout', follow_redirects=True)

    def add_message(self, text):
        """Records a message"""
        rv = self.app.post('/add_message', data={'text': text},
                                    follow_redirects=True)
        if text:
            assert 'Your message was recorded'.encode() in rv.data
        return rv

    # testing functions

    def test_register(self):
        """Make sure registering works"""
        rv = self.register('user1', 'default')
        assert 'You were successfully registered ' \
               'and can login now'.encode() in rv.data
        rv = self.register('user1', 'default')
        assert 'The username is already taken'.encode() in rv.data
        rv = self.register('', 'default')
        assert 'You have to enter a username'.encode() in rv.data
        rv = self.register('meh', '')
        assert 'You have to enter a password'.encode() in rv.data
        rv = self.register('meh', 'x', 'y')
        assert 'The two passwords do not match'.encode() in rv.data
        rv = self.register('meh', 'foo', email='broken')
        assert 'You have to enter a valid email address'.encode() in rv.data

    def test_login_logout(self):
        """Make sure logging in and logging out works"""
        rv = self.register_and_login('user1', 'default')
        assert 'You were logged in'.encode() in rv.data
        rv = self.logout()
        assert 'You were logged out'.encode() in rv.data
        rv = self.login('user1', 'wrongpassword')
        assert 'Invalid password'.encode() in rv.data
        rv = self.login('user2', 'wrongpassword')
        assert 'Invalid username'.encode() in rv.data

    def test_message_recording(self):
        """Check if adding messages works"""
        self.register_and_login('foo', 'default')
        self.add_message('test message 1'.encode())
        self.add_message('<test message 2>'.encode())
        rv = self.app.get('/')
        assert 'test message 1'.encode() in rv.data
        assert '&lt;test message 2&gt;'.encode() in rv.data

    def test_timelines(self):
        """Make sure that timelines work"""
        self.register_and_login('foo', 'default')
        self.add_message('the message by foo'.encode())
        self.logout()
        self.register_and_login('bar', 'default')
        self.add_message('the message by bar'.encode())
        rv = self.app.get('/public')
        assert 'the message by foo'.encode() in rv.data
        assert 'the message by bar'.encode() in rv.data

        # bar's timeline should just show bar's message
        rv = self.app.get('/')
        assert 'the message by foo'.encode() not in rv.data
        assert 'the message by bar'.encode() in rv.data

        # now let's follow foo
        rv = self.app.get('/foo/follow', follow_redirects=True)
        assert 'You are now following &#34;foo&#34;'.encode() in rv.data

        # we should now see foo's message
        rv = self.app.get('/')
        assert 'the message by foo'.encode() in rv.data
        assert 'the message by bar'.encode() in rv.data

        # but on the user's page we only want the user's message
        rv = self.app.get('/bar')
        assert 'the message by foo'.encode() not in rv.data
        assert 'the message by bar'.encode() in rv.data
        rv = self.app.get('/foo')
        assert 'the message by foo'.encode() in rv.data
        assert 'the message by bar'.encode() not in rv.data

        # now unfollow and check if that worked
        rv = self.app.get('/foo/unfollow', follow_redirects=True)
        assert 'You are no longer following &#34;foo&#34;'.encode() in rv.data
        rv = self.app.get('/')
        assert 'the message by foo'.encode() not in rv.data
        assert 'the message by bar'.encode() in rv.data


if __name__ == '__main__':
    unittest.main()
