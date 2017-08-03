import React, { Component } from 'react';

class Users extends Component {
  
  render() {
    const { isAuthenticated } = this.props.auth;
    return (
      <div className="container body-content">
        {
          isAuthenticated() && (
              <h3>Users</h3>
            )
        }
        
      </div>
    );
  }
}

export default Users;
