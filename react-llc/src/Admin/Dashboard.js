import React, { Component } from 'react';
import { Button }           from 'react-bootstrap';
import servicesBase         from '../Services/ServicesBase';
import moment               from 'moment';


class Dashboard extends Component {

    constructor(props) {
      super(props);
	  this.state = {
		stats: []
	  }
    }

    componentDidMount() {
      var that = this;
	  this.stats()
	    .then(function(stats) {
		  that.setState({stats: stats});
		});
    }
  
    render() {
	  let spin  = this.spinnerMarkup();
	  let stats = [];
	  if (this.state.stats.length > 0) {
	    for (var i = 0; i < this.state.stats.length; i++) {
		  var stat = this.state.stats[i];
		  let s = <div className="col-md-5 label-default dash-div">
            <h3 className="label-default dash-h3-label">{stat.title}</h3>

            <div className="label label-default dash-title">Total S3 Objects: </div>
            <div className="label label-primary dash-value">
			  {stat.objects}
            </div>
			
            <div className="dash-title-odd"> </div>
			
            <div className="label label-default dash-title">HTML Files: </div>
            <div className="label label-info dash-value">
              {stat.html}
            </div>

            <div className="dash-title-even"> </div>

            <div className="label label-default dash-title">Total Links: </div>
            <div className="label label-success dash-value">
              {stat.links}
            </div>
			
            <div className="dash-title-odd"> </div>
			
            <div className="label label-default dash-title">Last Extracted: </div>
            <div className="label label-info dash-value">
              {moment(stat.extracted).format("YYYY-MM-DD")}
            </div>

            <div className="dash-title-even"></div>

            <div className="label label-default dash-title">Invalid Links: </div>
            <div className="label label-danger dash-value">
              {stat.invalid}
            </div>
			
            <div className="dash-title-odd"> </div>
			
            <div className="label label-default dash-title">Last Checked: </div>
            <div className="label label-info dash-value">
              {moment(stat.checked).format("YYYY-MM-DD")}
            </div>
          </div>;
		  stats.push(s);  
	    }
	  }
	  
      return <div>
        <h2 className="bottom-20">Administrative Dashboard</h2>
        <div className="col-md-5 bottom-margin-10">
		  <Button
			className="btn btn-lg btn-default btn-mt"
			href='/#/admin/sources'
			>
			Sources
		  </Button>
		  <Button
			className="btn btn-lg btn-default btn-mt"
			href='/#/admin/settings'
			>
			Settings
	  	  </Button>
		  <Button
			className="btn btn-lg btn-default btn-mt"
			href='/#/admin/logs'
			>
			Logs
		  </Button>
		  <Button
			className="btn btn-lg btn-default btn-mt"
			href='/#/admin/users'
			>
			Users
		  </Button>
		  <Button
			className="btn btn-lg btn-default btn-mt"
			href='/#/admin/roles'
			>
			Roles
		  </Button>
	    </div>
		{ this.state.stats.length > 0 ?
		  <div>
		    {stats}
		  </div>
		  :
		  <div>
		    {spin}
		  </div>
		}
      </div>;
    }
}

export default servicesBase(Dashboard);
