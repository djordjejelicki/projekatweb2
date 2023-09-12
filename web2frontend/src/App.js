import { Fragment, useState } from "react";
import Header from "./components/Layout/Header";
import SigninForm from "./components/authorization/SigninForm";
import LoginForm from "./components/authorization/LoginForm";
import { AuthContextProvider } from "./Contexts/auth-context";

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
        {SignInForm && <SigninForm onClose={hideSignInFormHandler}/>}
        {LogInForm && <LoginForm onClose={hideLogInFormHandler}/>}
        <Header onSignIn = {showSignInFormHandler} onLogIn={showLogInFormHandler}/>
      </Fragment>
    </AuthContextProvider>
  );
}

export default App;
