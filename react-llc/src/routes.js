import { Route, Router } from 'react-router-dom';
import React        from 'react';
import App          from './App';
import Home         from './Home/Home';
import Report       from './Report/Report';
import InvalidLinks from './Report/InvalidLinks';
import WarningLinks from './Report/WarningLinks';
import Admin        from './Admin/Admin';
import Sources      from './Admin/Sources/Sources';
import ManageSource from './Admin/Sources/ManageSource';
import DeleteSource from './Admin/Sources/DeleteSource';
import Settings     from './Admin/Settings/Settings';
import Logs         from './Admin/Logs';
import Users        from './Admin/Users';
import Roles        from './Admin/Roles';
import Upload       from './Upload/Upload';
import Callback     from './Callback/Callback';
import Auth         from './Auth/Auth';
import history      from './history';

const auth = new Auth();

const handleAuthentication = (nextState, replace) => {
  if (/access_token|id_token|error/.test(nextState.location.hash)) {
    auth.handleAuthentication();
  }
}

export const makeMainRoutes = () => {
  return (
      <Router history={history} component={App}>
        <div>
          <Route       path="/"                               render={(props) => <App          auth={auth} {...props} />} />
          <Route exact path="/home"                           render={(props) => <Home         auth={auth} {...props} />} />
		  <Route exact path="/report"                         render={(props) => <Report       auth={auth} {...props} />} />
		  <Route exact path="/report/invalidlinks"            render={(props) => <InvalidLinks auth={auth} {...props} />} />
		  <Route exact path="/report/warninglinks"            render={(props) => <WarningLinks auth={auth} {...props} />} />
		  <Route exact path="/admin"                          render={(props) => <Admin        auth={auth} {...props} />} />
		  <Route exact path="/admin/sources"                  render={(props) => <Sources      auth={auth} {...props} />} />
		  <Route exact path="/admin/sources/manage"           render={(props) => <ManageSource auth={auth} {...props} />} />
		  <Route exact path="/admin/sources/manage/:id"       render={(props) => <ManageSource auth={auth} {...props} />} />
		  <Route exact path="/admin/sources/delete/:id"       render={(props) => <DeleteSource auth={auth} {...props} />} />
		  <Route exact path="/admin/settings"                 render={(props) => <Settings     auth={auth} {...props} />} />
		  <Route exact path="/admin/logs"                     render={(props) => <Logs         auth={auth} {...props} />} />
		  <Route exact path="/admin/users"                    render={(props) => <Users        auth={auth} {...props} />} />
		  <Route exact path="/admin/roles"                    render={(props) => <Roles        auth={auth} {...props} />} />
		  <Route exact path="/upload"                         render={(props) => <Upload       auth={auth} {...props} />} />
          <Route exact path="/callback"                       render={(props) => {
            handleAuthentication(props);
            return <Callback {...props} /> 
          }}/>
        </div>
      </Router>
  );
}
