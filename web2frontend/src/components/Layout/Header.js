import React, { Fragment, useContext } from "react"
import classes from "./Header.module.css";
import cartIcon from "../../assets/263142.png";
import Button from "../UI/Button/Button";

import AuthContext from "../../Contexts/auth-context";
import { Link, useNavigate } from "react-router-dom";
import HeaderCartButton from "./HeaderCartButton";

const Header = props => {

    const ctx = useContext(AuthContext);
    const navigate = useNavigate();

    const LogoutHandler = () => {
        navigate("/");
        ctx.onLogout();
    };

    return(
        <Fragment>
            <div className={classes["header-container"]}>
                <div className={classes["header-title-section"]}>                   
                        <img src={cartIcon} alt="shoping cart icon"/>
                        <label>Shop-@-Home</label>
                </div>
                <div className={classes["header-buttons-section"]}>
                    {ctx.isLoggedIn ? (
                        <Fragment>
                            <Link to="/">
                                <Button>Home</Button>
                            </Link>
                            <Button onClick={LogoutHandler}>Logout</Button>
                            <Link to="/profileinfo">
                                <Button>Profile infos</Button>
                            </Link>
                            {ctx.user.Role === 1 ? (
                                
                                   <HeaderCartButton onClick={props.onShowCart}/>
                                    
                            ) : null}
                            {ctx.user.Role === 2 && ctx.user.IsVerified ? (
                                <Fragment>
                                    <Link to="/addnewitem">
                                        <Button>Add Item</Button>
                                    </Link>
                                </Fragment>
                            ) : null}
                            {ctx.user.Role === 3 ? (
                                <Link to='/verification'>
                                    <Button>New Users</Button>
                                </Link>
                            ) : null}
                        </Fragment>
                    ) : (
                        <Fragment>
                            <Button onClick={props.onSignIn}>Signin</Button>
                            <Button onClick={props.onLogIn}>Login</Button>
                        </Fragment>
                    )}
                </div>
            </div>
        
        </Fragment>
    );
};

export default Header;