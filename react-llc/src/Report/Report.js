import React, { Component } from 'react';

class Report extends Component {
  login() {
    this.props.auth.login();
  }
  render() {
    const { isAuthenticated } = this.props.auth;
    return (
      <div className="container">
        {
          isAuthenticated() && (
		    <div>
              <h2 style={{"paddingBottom": "20px"}}>Report Dashboard</h2>
              <div className="col-md-5" style={{"paddingBottom": "10px", "margin": "10px"}}>
                <a style={{"marginRight": "5px"}} className="btn btn-lg btn-default" href="report/invalidlinks">Invalid Links</a>
                <a className="btn btn-lg btn-default" href="report/warninglinks">Warning Links</a>
              </div>

              <div style={{"clear": "both", "paddingTop": "20px"}}></div>
              <p></p>
			</div>
            )
        }
      </div>
    );
  }
}

export default Report;
