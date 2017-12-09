import React, { Component } from 'react';
import servicesBase         from '../../Services/ServicesBase';

const $     = require('jquery');
$.DataTable = require('datatables.net');

const columns = [
  {
	title: 'ID',
	data: 'id'
  },
  {
	title: 'Name',
	data: 'name'
  },
  {
	title: 'Value',
	data: 'value'
  },
  {
	title: '',
	data: 'id'
  },
  {
	title: 'Date Modified',
	data: 'modified'
  },
  {
	title: 'Modified By',
	data: 'user'
  },
  {
	title: '',
	data: 'id'
  }
];

class Settings extends Component {
	
  constructor(props) {
    super(props);
	this.state = {
      settings: [],
      loading: true
	}
  }
  
  componentDidMount() {
    this.settings();
	$(this.refs.main).DataTable({
      dom: '<"data-table-wrapper"t>',
      data: this.state.settings,
      columns,
      ordering: false,
      columnDefs: [
      {
        "render": function (data, type, row) {
	      return '<a title="' + data + '"><span class="badge truncate">' + data + '</span></a>';
        },
		"width": "100px",
        "targets": 0
      },
      {
        "render": function (data, type, row) {
          return '<a href="/admin/settings/editsetting/' + data +'" title="Edit this setting.">' +
                   '<i class="glyphicon glyphicon-pencil"></i>' +
                     '<span class="sr-only">Edit</span>' +
                 '</a>';
          },
          "targets": 3
      },
      {
	    "render": function (data, type, row) {
	      return '<span onclick="return confirm(\'Are you sure you wish to delete this setting? There is no undo.\')">' +
				   '<a href="/admin/settings/deletesetting/' + data + '" title="Remove this setting.">' +
				     '<i class="glyphicon glyphicon-remove" style="color: red;"></i>' +
				     '<span class="sr-only">Remove</span>' +
				   '</a>' +
			     '</span>';
	    },
	    "targets": 6
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
	if (this.state.settings.length > 0) {
	  const table = $('.data-table-wrapper')
                      .find('table')
                      .DataTable();
					
      table.clear();
      table.rows.add(this.state.settings);
	  table.columns.adjust();
      table.draw();
	  
	  this.changeSpinner(this, false);
	}
	
	return false;
  }
  
  render() {
	let spinner = this.spinnerMarkup();
    const { isAuthenticated } = this.props.auth;
    return (isAuthenticated() && (
		    <div className="container body-content">
              <h2 className="bottom-20">Settings</h2>
              <p>
                <a href="/admin/settings/createsetting">Create New</a>
              </p>
			  {spinner}
			  <table className="table" ref="main" />
              <div className="bottom-20">
                <a href="/admin">Back to Dashboard</a>
              </div>
            </div>));
  }
}

export default servicesBase(Settings);
