import React, { Component } from 'react';
import servicesBase         from '../Services/ServicesBase';

const $     = require('jquery');
$.DataTable = require('datatables.net');

const columns = [
  {
	title: 'ID',
	data: 'id'
  },
  {
	title: 'Source',
	data: 'source'
  },
  {
	title: 'Link Url',
	data: 'url'
  },
  {
	title: 'DateLastChecked',
	data: 'dateLastChecked'
  },
  {
	title: 'Date Last Found',
	data: 'dateLastFound'
  },
  {
	title: 'Attempt Count',
	data: 'attemptCount'
  },
  {
	title: 'Bucket Locations',
	data: 'id'
  },
  {
	title: 'Reset Link',
	data: 'id'
  }
];

class InvalidLinks extends Component {
	
  constructor(props) {
    super(props);
	this.state = {
	  invalidLinks: [],
      loading: true
	}
  }
  
  componentDidMount() {
	var that = this;
	$(this.refs.main).DataTable({
      //dom: '<"data-table-wrapper"t>',
      columns,
      ordering: true,
	  processing: true,
      serverSide: true,
	  stateSave: true,
	  autoWidth: false,
	  lengthChange: true,
      filter: true,
      info: true,
	  ajax: function(data, callback, settings) {
	    that.invalidLinks(data).then(function(r) {
		  var h = {};
		  h.draw = data.draw;  
          h.data = r.data.data;
          h.recordsTotal = r.data.recordsTotal;
          h.recordsFiltered = r.data.recordsFiltered;
		  callback(h);
		});
	  },
	  columnDefs: [
	  {
		render: function (data, type, row) {
			var url = data.replace("http://", "").replace("https://", "").split("/")[0];
			return '<a href="'+data+'" target="_blank">'+url+'</a>';
		},
		targets: 2
	  },
	  {
		render: function (data, type, row) {
			return '<a href="/Report/BucketLocations/' + data + '" title="' + data + '" target="_blank" class="btn btn-info" data-toggle="modal" data-target="#myModal">' + data + '</a>';
		},
		targets: 6
	  },
	  {
		render: function (data, type, row) {
			return '<a href="/Report/ResetLink/' + data + '" title="' + data + '" class="btn btn-info glyphicon glyphicon-trash" onclick="return confirm(\'Are you sure? A link reset cannot be un-done\');"></a>';
		},
		targets: 7
	  }
	  ]
    });
  }
  
  componentWillUnmount(){
    $('.data-table-wrapper')
      .find('table')
      .DataTable()
      .destroy(true);
  }
  
  shouldComponentUpdate() {
	if (this.state.invalidLinks.length >= 0) {
	  const table = $('.data-table-wrapper')
                      .find('table')
                      .DataTable();
					
      //table.clear();
      //table.rows.add(this.state.invalidLinks);
	  table.columns.adjust();
      table.draw();
	  
	  this.turnOffSpinner(this);
	}
	
	return false;
  }
  
  render() {
	let spinner = this.spinnerMarkup();
    const { isAuthenticated } = this.props.auth;
    return (isAuthenticated() && (
		    <div className="container body-content">
              <h2 className="bottom-20">Invalid Links</h2>
			  {spinner}
			  <table width="100%" className="table table-striped table-bordered" ref="main" cellSpacing="0" />
              <div className="bottom-20">
                <a href="/report">Back to Dashboard</a>
              </div>
            </div>));
  }
}

export default servicesBase(InvalidLinks);
