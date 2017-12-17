import React, { Component } from 'react';
import { Navbar, Button } from 'react-bootstrap';
import Content from './Home/Content';
import NoAuth  from './Auth/NoAuth';
import './App.css';

class App extends Component {

  constructor(props) {
    super(props);

    var year = new Date().getFullYear();
    document.getElementsByTagName('footer')[0]
        .innerHTML = '<p>&copy; ' + year + ' - Idaho Digital Learning</p>';
  }

  goTo(route) {
    this.props.history.replace(`/${route}`)
  }

  login() {
    this.props.auth.login();
  }

  logout() {
    this.props.auth.logout();
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
                onClick={this.goTo.bind(this, 'home')}
              >
                Home
              </Button>
              <Button
                bsStyle="primary"
                className="btn-margin"
                onClick={this.goTo.bind(this, 'report')}
              >
                Reports
              </Button>
			  <Button
                bsStyle="primary"
                className="btn-margin"
                onClick={this.goTo.bind(this, 'admin')}
              >
                Admin
              </Button>
		      <Button
                bsStyle="primary"
                className="btn-margin"
                onClick={this.goTo.bind(this, 'admin/upload')}
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
