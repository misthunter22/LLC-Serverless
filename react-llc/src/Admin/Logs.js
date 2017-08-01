import React, { Component } from 'react';
import NoAuth  from '../Auth/NoAuth';

class Logs extends Component {
  
  render() {
    const { isAuthenticated } = this.props.auth;
	let content = null;
	if (!isAuthenticated()) {
      content =
        <NoAuth {...this.props} />;
    }
	else {
	  content = 
        <h3>Logs</h3>;
	}
    return (
	  <div className="container body-content">
	    {content}
	  </div>
    );
  }
}

export default Logs;
