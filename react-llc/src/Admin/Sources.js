import React, { Component } from 'react';
import NoAuth  from '../Auth/NoAuth';

class Sources extends Component {
  
  render() {
    const { isAuthenticated } = this.props.auth;
	let content = null;
	if (!isAuthenticated()) {
      content =
        <NoAuth {...this.props} />;
    }
	else {
	  content = 
        <h3>Sources</h3>;
	}
    return (
	  <div className="container body-content">
	    {content}
	  </div>
    );
  }
}

export default Sources;
