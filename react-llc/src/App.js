import React, { Component } from 'react';
import { Navbar, Button, Nav, NavItem } from 'react-bootstrap';
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
		<Navbar fluid>
		  <Navbar.Header>
			<Navbar.Brand>
			  <a href="#">LLC</a>
			</Navbar.Brand>
			<Navbar.Toggle />
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
		  </Navbar.Header>
		  <Navbar.Collapse>
			<Nav>
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
                    onClick={this.goTo.bind(this, 'home')}
                >
                    Home2
                </Button>
			</Nav>
		  </Navbar.Collapse>
		</Navbar>
	  </div>
    );
  }
}

export default App;
