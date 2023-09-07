import React, { useReducer, useRef } from "react";
import Modal from "../UI/Modal/Modal";
import Input from "../UI/Input/Input";
import Button from "../UI/Button/Button";
import classes from "./Form.module.css";

const emailReducer = (state, action) => {

    if(action.type === 'USER_INPUT'){
        return { value: action.val, isValid: action.val.includes("@") };
    }
    
    if (action.type === "INPUT_BLUR"){
        return {value: state.value, isValid: state.value.includes("@") };
    }

    return {value: "", isValid: false}
};

const passwordReducer = (state, action) => {

    if(action.type === "USER_INPUT"){
        return {value: action.val, isValid: action.val.trim().lenght > 6};
    }

    if(action.type === "INPUT_BLUR"){
        return {value: state.value, isValid: state.value.trim().lenght > 6};
    }

    return {value: "", isValid: false};
};




const LoginForm = props => {


    const [emailState, dispatchEmail] = useReducer(emailReducer, {value: "", isValid: null});
    const [passwordState, dispatchPassword] = useReducer(passwordReducer, {value: "", isValid: null});

    const emailInputRef = useRef();
    const passwordInputRef = useRef();

    const {isValid: emailIsValid} = emailState;
    const {isValid: passwordIsValid} = passwordState;

    const emailChangeHandler = event => {
        dispatchEmail({type: "USER_INPUT", val: event.target.value});
    };

    const validateEmailHandler = () => {
        dispatchEmail({type: "INPUT_BLUR"});
    };

    const passwordChangeHandler = event => {
        dispatchPassword({type: "USER_INPUT", val: event.target.value});
    };

    const validatePasswordHandler = () => {
        dispatchPassword({type: "INPUT_BLUR"});
    };



    return(
        <Modal onClose={props.onClose} className={classes.form}>
            <center>
                <p className={classes["login-title"]}>Login</p>
            </center>
            <form>
                <Input ref={emailInputRef} id="email" label="Email" type="email" isValid={emailIsValid} value={emailState.value} onChange={emailChangeHandler} onBlur={validateEmailHandler}/>
                <Input ref={passwordInputRef} id="password" label="Password" type="password" isValid={passwordIsValid} value={passwordState.value} onChange={passwordChangeHandler} onBlur={validatePasswordHandler}/>
                <div className={classes.actions}>
                    <Button type="submit" id="login">Login</Button>
                    <Button onClick={props.onClose}>Close</Button>
                </div>
            </form>
        </Modal>
    );
};

export default LoginForm;