import { Fragment, useState } from "react";
import Header from "./components/Layout/Header";
import SigninForm from "./components/authorization/SigninForm";
import LoginForm from "./components/authorization/LoginForm";
import { AuthContextProvider } from "./Contexts/auth-context";
import { Route, Routes, BrowserRouter as Router } from "react-router-dom";
import Dashboard from "./components/Dashboard/Dashboard";
import Profileinfo from "./common/Profileinfo";
import Verification from "./common/Admin/Verification";
import NewItem from "./common/Seller/NewItem";
import { ItemContextProvider } from "./Contexts/item-context";
import CartProvider from "./Contexts/CartProvider";
import Cart from "./components/Cart/Cart";


function App() {

  const [SignInForm, setSignInForm] = useState(false);
  const [LogInForm, setLogInForm] = useState(false);
  const [Cartshown, setCart] = useState(false);

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

  const showCartHandler = () => {
    setCart(true);
  }

  const hideCartHandler = () => {
    setCart(false);
  }

  return (
    <AuthContextProvider>
      <ItemContextProvider>
        <CartProvider>
        <Fragment>
          <Router>
          
              {SignInForm && <SigninForm onClose={hideSignInFormHandler}/>}
              {LogInForm && <LoginForm onClose={hideLogInFormHandler}/>}
              {Cartshown  &&  <Cart onClose={hideCartHandler}/>}
              <Header onShowCart = {showCartHandler} onSignIn = {showSignInFormHandler} onLogIn={showLogInFormHandler}/>
              <main>
                <Routes>
                  <Route path="/" exact element={<Dashboard/>}/>
                  <Route path="/profileinfo" element={<Profileinfo/>}/>
                  <Route path="/verification" element={<Verification/>}/>
                  <Route path="/addnewitem" element={<NewItem/>}/>
                </Routes>             
              </main>
          </Router>
        </Fragment>
        </CartProvider>
      </ItemContextProvider>
    </AuthContextProvider>
  );
}

export default App;
