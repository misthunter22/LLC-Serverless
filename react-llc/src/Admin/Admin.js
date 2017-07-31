import React, { Component } from 'react';
import Dashboard from './Dashboard';

class Admin extends Component {
  login() {
    this.props.auth.login();
  }
  render() {
    const { isAuthenticated } = this.props.auth;
    return (
      <div className="container body-content">
        {
          isAuthenticated() && (
              <Dashboard {...this.props} />
            )
        }
        
      </div>
    );
  }
}

export default Admin;
