import React, { useEffect, useRef, useState } from "react";
import Button from "../UI/Button/Button";
import Input from "../UI/Input/Input";
import classes from "./Form.module.css";
import Modal from "../UI/Modal/Modal";
import axios from "axios";


const SigninForm = props => {

    const emailInputRef = useRef();
    const passwordInputRef = useRef();
    const passwordRepeatInputRef = useRef();


    const [enteredEmail, setEnteredEmail] = useState("");
    const [enteredPassword, setEnteredPassword] = useState("");
    const [enteredPasswordRepeat, setEnteredPasswordRepeat] = useState("");

    const [emailIsValid, setEmailIsValid] = useState();
    const [passwordIsValid, setPasswordIsValid] = useState();
    const [passwordRepeatIsValid, setRepeatPasswordIsValid] = useState();
    const [formIsValid, setFormIsValid] = useState(false);

    useEffect(() =>{
        const identifier = setTimeout(() => {setFormIsValid(emailIsValid && passwordIsValid && passwordRepeatIsValid);},500);
        return () => {clearTimeout(identifier);};
    },[emailIsValid,passwordIsValid,passwordRepeatIsValid]);

    const emailChangeHandler = event => {
        setEnteredEmail(event.target.value);
        const regex = /^[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,}$/i;
        setEmailIsValid(regex.test(event.target.value));
        setFormIsValid(event.target.value.includes('@') && passwordIsValid && passwordRepeatIsValid);
    };

    const passwordChangeHandler = event => {
        setEnteredPassword(event.target.value);
        const regex = /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$/;
        setPasswordIsValid(regex.test(event.target.value));
        setFormIsValid(emailIsValid && passwordIsValid && passwordRepeatIsValid);
    };

    const passwordRepeatChangeHandler = event => {
        setEnteredPasswordRepeat(event.target.value);
        const regex = /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$/;
        const isValid = regex.test(event.target.value) && event.target.value === enteredPassword;
        setRepeatPasswordIsValid(isValid);
        setFormIsValid(emailIsValid && passwordIsValid && passwordRepeatIsValid);
    };

    const submitHandler = async(event) => {

        event.preventDefault();
        const formData = new FormData();

        if(formIsValid){
            const button = document.getElementById('signin');
            button.textContent = "Signing in";
            button.disabled = true;

            formData.append('UserName', event.target.username.value);
            formData.append('FirstName',event.target.firstname.value);
            formData.append('LastName',event.target.lastname.value);
            formData.append('Email',enteredEmail);
            formData.append('Address',event.target.address.value);
            formData.append("BirthDate",event.target.birthdate.value);
            formData.append("Password", enteredPassword);
            if(event.target.avatar.files.lenght > 0){
                formData.append("file", event.target.avatar.files[0]);
            }

            let selectedOption = event.target.elements.accType.value;
            try{
                if(selectedOption === 'Buyer'){
                    const response = await axios.post(process.env.REACT_APP_SERVER_URL + 'users/signinBuyer', formData, {headers:{'Content-Type' : 'multipart/form-data'}});
                    alert(response.data);
                    props.onClose();
                }
                else{
                    const response = await axios.post(process.env.REACT_APP_SERVER_URL + 'users/signinSeller', formData, {headers:{'Content-Type' : 'multipart/form-data'}});
                    alert(response.data);
                    props.onClose();
                }
            }
            catch (error){
                alert(error.response.data.detail);
                button.textContent = "Signin";
                button.disabled = false;
            }
        }
        else if(!emailIsValid){
            emailInputRef.current.focus();
        }
        else if(!passwordIsValid){
            passwordInputRef.current.focus();
        }
        else if(!passwordRepeatIsValid){
            passwordRepeatInputRef.current.focus();
        }
    };

    return(
        <Modal onClose={props.onClose} className={classes.form}>
            <center>
                <p className={classes["signin-title"]}>Signin</p>
            </center>
            <form onSubmit={submitHandler}>
                <Input id="username" label="Username" type="text"/>
                <Input id="firstname" label="Firstname" type="text"/>
                <Input id="lastname" label="Lastname" type="text"/>
                <Input ref={emailInputRef} id="email" label="Email" type="email" isValid={emailIsValid} value={enteredEmail} onChange={emailChangeHandler}/>
                <Input id="address" label="Address" type="text"/>
                <Input id="birthdate" label="Birth Date" type="date"/>
                <Input ref={passwordInputRef} id="password" label="Password" type="password" isValid={passwordIsValid} value={enteredPassword} onChange={passwordChangeHandler}/>
                <Input ref={passwordRepeatInputRef} id="passwordRepeat" label="Repeat Password" type="password" isValid={passwordRepeatIsValid} value={enteredPasswordRepeat} onChange={passwordRepeatChangeHandler}/>
                <input className={classes["radio-button"]} type="radio" value="Buyer" name="accType"  defaultChecked/>Buyer
                <input type="radio" value="Seller" name="accType"/>Seller
                <Input type='file' label="Profile picture" id="avatar"/>
                <div className={classes.actions}>
                    <Button type='submit' id='signin'>Signin</Button>
                    <Button onClick={props.onClose}>Close</Button>
                </div>

            </form>
        </Modal>
    );
};

export default SigninForm;