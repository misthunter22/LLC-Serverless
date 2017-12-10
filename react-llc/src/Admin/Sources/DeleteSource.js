import React, { Component } from 'react';
import servicesBase         from '../../Services/ServicesBase';

class DeleteSource extends Component {
  constructor(props) {
    super(props);
	this.state = {
      message: null,
	}
  }
	
  componentDidMount() {
	if (this.props.match.params.id) {
	  var that = this;
	  this.changeSource(
	  {
		Id: this.props.match.params.id,
		Delete: true
      })
	  .then(function(result) {
		if (result.status) {
	      window.location = "/admin/sources"
		}
		else {
		  that.setState({message: "Delete Source Error!"});
		}
	  });
	}
  }
  
  render() {
    const { isAuthenticated } = this.props.auth;
    return (
      <div className="container body-content">
        {
          isAuthenticated() && (
              <h3>{this.state.message}</h3>
            )
        }
        
      </div>
    );
  }
}

export default servicesBase(DeleteSource);
