import React, { Component } from 'react';
import { Navbar, Nav, NavItem } from 'react-bootstrap';
import { Switch, Route } from 'react-router';

import Home from './pages/Home';
import Report from './pages/Report';
import Admin from './pages/Admin';
import Upload from './pages/Upload';

import logo from './logo.png';
import './App.css';

class App extends Component {
  render() {
    return (
        <main>
            <Navbar>
                <Navbar.Header>
                    <Navbar.Brand>
                        <a href="/">LLC</a>
                    </Navbar.Brand>
                    <Navbar.Toggle />
                </Navbar.Header>
                <Navbar.Collapse>
                    <Nav>
                        <NavItem eventKey={1} href="/">Home</NavItem>
                        <NavItem eventKey={2} href="/report">Report</NavItem>
                        <NavItem eventKey={3} href="/admin">Admin</NavItem>
                        <NavItem eventKey={4} href="/upload">Upload</NavItem>
                    </Nav>
                </Navbar.Collapse>
            </Navbar>
            <Switch>
                <Route exact path='/' component={Home}/>
                <Route path='/report' component={Report}/>
                <Route path='/admin' component={Admin}/>
                <Route path='/upload' component={Upload}/>
            </Switch>
        </main>
    );
  }
}

export default App;
