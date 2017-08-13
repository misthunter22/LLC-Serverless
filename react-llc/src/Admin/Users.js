import React, { Component } from 'react';

class Users extends Component {
  
  render() {
    const { isAuthenticated } = this.props.auth;
    return (
      <div className="container body-content">
        {
          isAuthenticated() && (
              <div>
			    <h3>Users</h3>
				<span>User management is handled through <a href="https://auth0.com/" target="_blank">Auth0</a>. 
				      Groups, roles and permissions are applied when the user authenticates against the datastores 
					  defined in <a href="https://auth0.com/" target="_blank">Auth0</a>, which then passes any permissions
					  tokens through.
				</span>
			  </div>
            )
        }
        
      </div>
    );
  }
}

export default Users;
