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
                                <Fragment>
                                    <Link to="/myOrders">
                                        <Button>My orders</Button>
                                    </Link>
                                   <HeaderCartButton onClick={props.onShowCart}/>
                                </Fragment>    
                            ) : null}
                            {ctx.user.Role === 2 && ctx.user.IsVerified ? (
                                <Fragment>
                                    <Link to="/addnewitem">
                                        <Button>Add Item</Button>
                                    </Link>
                                    <Link to="/newOrders">
                                        <Button>New Orders</Button>
                                    </Link>
                                    <Link to="/orderHistory">
                                        <Button>Order History</Button>
                                    </Link>
                                </Fragment>
                            ) : null}
                            {ctx.user.Role === 3 ? (
                                <Fragment>
                                <Link to='/verification'>
                                    <Button>New Users</Button>
                                </Link>
                                <Link to='/allOrders'>
                                    <Button>All Orders</Button>
                                </Link>
                                </Fragment>
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