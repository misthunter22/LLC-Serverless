import React, { Component } from 'react';
import Content from './Content';

class Home extends Component {
  
  render() {
	const { isAuthenticated } = this.props.auth;
	
    return (
	  <div className="container">
      {
        isAuthenticated() && (
          <Content />
        )
      }
      </div>
    );
  }
}

export default Home;
