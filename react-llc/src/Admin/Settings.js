import React, { Component } from 'react';

class Settings extends Component {
  
  render() {
    const { isAuthenticated } = this.props.auth;
    return (
      <div className="container body-content">
        {
          isAuthenticated() && (
              <h3>Settings</h3>
            )
        }
        
      </div>
    );
  }
}

export default Settings;
