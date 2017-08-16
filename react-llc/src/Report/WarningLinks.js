import React, { Component } from 'react';

class WarningLinks extends Component {
  
  render() {
    const { isAuthenticated } = this.props.auth;
    return (
      <div className="container body-content">
        {
          isAuthenticated() && (
		    <div>
              <h3>Warning Links</h3>
			</div>
            )
        }
        
      </div>
    );
  }
}

export default WarningLinks;
