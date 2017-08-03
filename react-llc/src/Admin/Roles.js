import React, { Component } from 'react';

class Roles extends Component {
  
  render() {
    const { isAuthenticated } = this.props.auth;
    return (
      <div className="container body-content">
        {
          isAuthenticated() && (
              <h3>Roles</h3>
            )
        }
        
      </div>
    );
  }
}

export default Roles;
