import React, { Component } from 'react';

class DeleteSource extends Component {
  
  render() {
    const { isAuthenticated } = this.props.auth;
    return (
      <div className="container body-content">
        {
          isAuthenticated() && (
              <h3>Delete Source</h3>
            )
        }
        
      </div>
    );
  }
}

export default DeleteSource;
