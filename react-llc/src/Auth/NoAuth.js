import React, { Component } from 'react';

class NoAuth extends Component {
  login() {
    this.props.auth.login();
  }
  
  render() {
    return (
      <h4>
        You are not logged in! Please{' '}
        <a
          style={{ cursor: 'pointer' }}
          onClick={this.login.bind(this)}
        >
          Log In
        </a>
        {' '}to continue.
      </h4>
    );
  }
}

export default NoAuth;
