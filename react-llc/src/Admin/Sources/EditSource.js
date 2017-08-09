import React, { Component } from 'react';

class EditSource extends Component {
  
  render() {
    const { isAuthenticated } = this.props.auth;
    return (
      <div className="container body-content">
        {
          isAuthenticated() && (
              <h3>Edit Source</h3>
            )
        }
        
      </div>
    );
  }
}

export default EditSource;
