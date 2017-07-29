import React, { Component } from 'react';

class Report extends Component {
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
                You are logged in! Report
              </h4>
            )
        }
      </div>
    );
  }
}

export default Report;
