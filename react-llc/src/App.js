import React, { Component } from 'react';
import { Navbar, Button } from 'react-bootstrap';
import './App.css';

class App extends Component {
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

    return (
      <div>
		<Navbar>
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
                onClick={this.goTo.bind(this, 'upload')}
              >
                Upload
              </Button>
			  {
			    !isAuthenticated() && (
				  <Button
					bsStyle="primary"
					className="btn-margin"
					onClick={this.login.bind(this)}
				  >
					Log In
				  </Button>
			    )
			  }
			  {
			   isAuthenticated() && (
				  <Button
					bsStyle="primary"
					className="btn-margin"
					onClick={this.logout.bind(this)}
				  >
					Log Out
				  </Button>
				)
			  }
		  </Navbar.Collapse>
		</Navbar>
	  </div>
    );
  }
}

export default App;
