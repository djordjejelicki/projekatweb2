import { Fragment, useState } from "react";
import Header from "./components/Layout/Header";
import SigninForm from "./components/authorization/SigninForm";
import LoginForm from "./components/authorization/LoginForm";
import { AuthContextProvider } from "./Contexts/auth-context";
import { Route, Routes, BrowserRouter as Router } from "react-router-dom";
import Dashboard from "./components/Dashboard/Dashboard";
import Profileinfo from "./common/Profileinfo";

function App() {

  const [SignInForm, setSignInForm] = useState(false);
  const [LogInForm, setLogInForm] = useState(false);

  const showLogInFormHandler = () => {
    setLogInForm(true);
  };

  const hideLogInFormHandler = () => {
    setLogInForm(false);
  };

  const showSignInFormHandler = () => {
    setSignInForm(true);
  };

  const hideSignInFormHandler = () => {
    setSignInForm(false);
  };

  return (
    <AuthContextProvider>
      <Fragment>
        <Router>
            {SignInForm && <SigninForm onClose={hideSignInFormHandler}/>}
            {LogInForm && <LoginForm onClose={hideLogInFormHandler}/>}
            <Header onSignIn = {showSignInFormHandler} onLogIn={showLogInFormHandler}/>
            <main>
              <Routes>
                <Route path="/" exact element={<Dashboard/>}/>
                <Route path="/profileinfo" element={<Profileinfo/>}/>
              </Routes>             
            </main>
        </Router>
      </Fragment>
    </AuthContextProvider>
  );
}

export default App;
