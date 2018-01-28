import React, { Component } from 'react';
import { Navbar, Button }   from 'react-bootstrap';
import Content              from './Home/Content';
import NoAuth               from './Auth/NoAuth';
import Auth                 from './Auth/Auth';
import './App.css';

const auth = new Auth();

class App extends Component {

  constructor(props) {
    super(props);

    var year = new Date().getFullYear();
    document.getElementsByTagName('footer')[0]
        .innerHTML = '<p>&copy; ' + year + ' - Idaho Digital Learning</p>';
		
	if (/access_token|id_token|error/.test(window.location.hash)) {
		auth.handleAuthentication();
	}
  }

  login() {
    auth.login();
  }

  logout() {
    auth.logout();
  }

  render() {
    const { isAuthenticated } = this.props.auth;

	let content = null;
    if (isAuthenticated() && this.props.location.pathname === "/") {
      content =
	    <div className="container body-content">
          <Content />
		</div>;
    } else if (!isAuthenticated()) {
      content =
	    <div className="container body-content">
          <NoAuth {...this.props} />
		</div>;
    }
	
	let userName = this.props.auth.idTokenData('name');

    return (
      <div>
		<Navbar fixedTop={true}>
		  <Navbar.Header>
			<Navbar.Brand>
			  <a href="/">LLC</a>
			</Navbar.Brand>
			<Navbar.Toggle />
		  </Navbar.Header>
		  <Navbar.Collapse>
              <Button
                bsStyle="primary"
                className="btn-margin"
                href='/#/home'
              >
                Home
              </Button>
              <Button
                bsStyle="primary"
                className="btn-margin"
                href='/#/report'
              >
                Reports
              </Button>
			  <Button
                bsStyle="primary"
                className="btn-margin"
                href='/#/admin'
              >
                Admin
              </Button>
		      <Button
                bsStyle="primary"
                className="btn-margin"
                href='/#/admin/upload'
              >
                Upload
              </Button>
			  		{
          !isAuthenticated() && (
            <Button
			  bsStyle="primary"
			  className="btn-margin float-nav"
			  onClick={this.login.bind(this)}
			>
			  Log In
		    </Button>
	      )
		  }
	      {
		    isAuthenticated() && (
		      <div className="float-nav">
		        <span>Welcome, {userName}! </span>
		        <Button
			      bsStyle="primary"
				  className="btn-margin"
				  onClick={this.logout.bind(this)}
			    >
			      Log Out
			    </Button>
			  </div>
		    )
		  }
		  </Navbar.Collapse>
		</Navbar>
		{content}
	  </div>
    );
  }
}

export default App;
