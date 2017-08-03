import React, { Component } from 'react';

class Logs extends Component {
  
  render() {
    const { isAuthenticated } = this.props.auth;
    return (
      <div className="container body-content">
        {
          isAuthenticated() && (
              <h3>Logs</h3>
            )
        }
        
      </div>
    );
  }
}

export default Logs;
