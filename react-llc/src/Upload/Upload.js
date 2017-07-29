import React, { Component } from 'react';

class Upload extends Component {
  login() {
    this.props.auth.login();
  }
  render() {
    const { isAuthenticated } = this.props.auth;
    return (
      <div className="container">
        {
          isAuthenticated() && (
              <h4>
                You are logged in! Upload
              </h4>
            )
        }
      </div>
    );
  }
}

export default Upload;
