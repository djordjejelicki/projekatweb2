import React, { Fragment } from "react"
import classes from "./Header.module.css";
import cartIcon from "../../assets/263142.png";
import headerPicture from "../../assets/headerPicture3.jpg";
import Button from "../UI/Button/Button";

const Header = props => {
    return(
        <Fragment>
            <div className={classes["header-container"]}>
                <div className={classes["header-title-section"]}>
                    
                        <img src={cartIcon} alt="shoping cart icon"/>
                        <label>Shop-@-Home</label>
                </div>
                <div className={classes["header-buttons-section"]}>
                    <Button onClick={props.onSignIn}>Signin</Button>
                    <Button onClick={props.onLogIn}>Login</Button>
                </div>
            </div>
            <div className={classes["main-image"]}>
                <img src={headerPicture} alt="shop"/>
            </div>
        </Fragment>
    );
};

export default Header;