import React, { Component } from 'react';
import servicesBase         from '../../Services/ServicesBase';

class DeleteSetting extends Component {
  constructor(props) {
    super(props);
	this.state = {
      message: null,
	}
  }
	
  componentDidMount() {
	if (this.props.match.params.id) {
	  var that = this;
	  this.change('/api/settings',
	    {
		  Id: this.props.match.params.id,
		  Delete: true
		})
	    .then(function(result) {
		  if (result.status) {
	        window.location = "/admin/settings"
		  }
		  else {
			that.setState({message: "Delete Settings Error!"});
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

export default servicesBase(DeleteSetting);
