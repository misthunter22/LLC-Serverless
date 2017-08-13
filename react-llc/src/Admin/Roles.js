import React, { Component } from 'react';

class Roles extends Component {
  
  render() {
    const { isAuthenticated } = this.props.auth;
    return (
      <div className="container body-content">
        {
          isAuthenticated() && (
              <div>
			    <h3>Roles</h3>
				<p>User management is handled through <a href="https://auth0.com/" rel="noopener noreferrer" target="_blank">Auth0</a>. 
				   Groups, roles and permissions are applied when the user authenticates against the datastores 
				   defined in <a href="https://auth0.com/" rel="noopener noreferrer" target="_blank">Auth0</a>, which then passes any permissions
				   tokens through.
				</p>
				<p>
				  The current roles that exist in the application are:
				  <ul>
				    <li>Admin</li>
					<li>ReportUser</li>
				  </ul>
				</p>
			  </div>
            )
        }
        
      </div>
    );
  }
}

export default Roles;
