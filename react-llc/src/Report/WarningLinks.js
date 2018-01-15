import React, { Component } from 'react';
import servicesBase         from '../Services/ServicesBase';
import { resetLink }        from '../Services/ServicesBase';
import $                    from 'jquery';

window.jQuery = $;
window.$ = $;
global.jQuery = $;

require('bootstrap');
require('datatables.net');
require('datatables-bootstrap');

const columns = [
  {
	title: 'ID',
	data: 'id'
  },
  {
	title: 'Content Size',
	data: 'contentSize'
  },
  {
	title: 'Mean',
	data: 'mean'
  },
  {
	title: 'Standard Deviation',
	data: 'standardDeviation'
  },
  {
	title: 'SD Range',
	data: 'sdMaximum'
  },
  {
	title: 'Link ID',
	data: 'url'
  },
  {
	title: 'Compare Screenshots',
	data: 'link',
	sortable: false
  },
  {
	title: 'Bucket Locations',
	data: 'link',
	sortable: false
  },
  {
	title: 'Reset Link',
	data: 'link',
	sortable: false
  }
];

window.warning_reset_link = function(id) {
  var check = window.confirm("Are you sure? A link reset cannot be un-done");
  if (check === true) {
    resetLink(id).then(function(data) {
      window.location.reload();
    })
    .catch(function(result) {
	  console.log(result);
    });
  }
}

class WarningLinks extends Component {
	
  constructor(props) {
    super(props);
	this.state = {
	  warningLinks: [],
      loading: true
	}
  }
  
  componentDidMount() {
    var that  = this;
	var table = $(this.refs.main).DataTable({
      columns,
	  dom: 'lfrtip',
      ordering: true,
	  processing: false,
      serverSide: true,
	  stateSave: true,
	  autoWidth: false,
	  lengthChange: true,
      filter: true,
      info: true,
	  ajax: function(data, callback, settings) {
		that.changeSpinner(that, true);
	    that.warningLinks(data)
		  .then(function(r) {
		    that.changeSpinner(that, false);
		    var h = {};
		    h.draw = data.draw;  
            h.data = r.data;
            h.recordsTotal = r.recordsTotal;
            h.recordsFiltered = r.recordsFiltered;
		    callback(h);
		  });
	  },
      columnDefs: [
      {
        render: function (data, type, row) {
          return '<a href="' + data + '" title="' + data + '" target="_blank" class="label label-success">' + row["link"].slice(0, 8) + '...</a>';
        },
        targets: 5
      },
	  {
		render: function (data, type, row) {
		  return '<a data-src="' + data + '" data-type="link" title="' + data + '" class="btn btn-info bucket-screenshots" data-toggle="modal" data-target="#myModal">' + data.slice(0, 8) + '...</a>';
		},
		targets: 6
	  },
	  {
		render: function (data, type, row) {
		  return '<a data-src="' + data + '" data-type="link" title="' + data + '" class="btn btn-info bucket-locations" data-toggle="modal" data-target="#myModal">' + data.slice(0, 8) + '...</a>';
		},
		targets: 7
	  },
	  {
	    render: function (data, type, row) {
		  return '<a onclick="warning_reset_link(\'' + data + '\')" title="' + data + '" class="btn btn-info glyphicon glyphicon-trash"></a>';
		},
		targets: 8
	  }
	  ]
    });
	
	table
      .on('draw.dt', function () {
        that.configureScrenshotLinks();
      });
	  
    this.configureScrenshotLinks(); 
  }
  
  componentWillUnmount(){
    $('.data-table-wrapper')
      .find('table')
      .DataTable()
      .destroy(true);
  }
  
  shouldComponentUpdate() {
	if (this.state.warningLinks.length >= 0) {
	  const table = $('.data-table-wrapper')
                      .find('table')
                      .DataTable();
					
	  table.columns.adjust();
      table.draw();
	}
	
	return false;
  }
  
  render() {
	let spinner = this.spinnerMarkup();
    const { isAuthenticated } = this.props.auth;
    return (isAuthenticated() && (
		    <div className="container body-content">
              <h2 className="bottom-20">Warning Links</h2>
			  {spinner}
			  <table width="100%" className="table table-striped table-bordered" ref="main" cellSpacing="0">
			  </table>
              <div className="bottom-20">
                <a href="/report">Back to Dashboard</a>
              </div>
            </div>));
  }
}

export default servicesBase(WarningLinks);
