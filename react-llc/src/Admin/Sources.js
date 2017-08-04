import React, { Component } from 'react';
import servicesBase from '../Services/ServicesBase';

class Sources extends Component {
	
  constructor(props) {
    super(props);
	this.state = {
      sources: []
	}
  }
  
  componentDidMount() {
    this.sources(true);
  }
  
  render() {
    const { isAuthenticated } = this.props.auth;
    return (isAuthenticated() && (
		    <div className="container body-content">
              <h2 className="bottom-20">Sources</h2>
              <p>
                A Tag CreateSource
              </p>
              <table className="table">
			    <tbody>
                  <tr className="border-bottom-silver">
                    <th>ID</th>
                    <th>Name</th>
                    <th>Allow Link Checking</th>
                    <th>Allow Link Extractions</th>
                    <th>Object Source Name</th>
                    <th></th>
                    <th>Date Created</th>
                    <th></th>
                  </tr>
				</tbody>
              </table>

              <div className="bottom-20">
                A Tag Back to Dashboard
              </div>
            </div>));
  }
}

export default servicesBase(Sources);
