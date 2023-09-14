import React, { Fragment, useContext, useRef, useState } from "react";
import classes from "./Profileinfo.module.css";
import { useNavigate } from "react-router-dom";
import Button from "../components/UI/Button/Button";
import AuthContext from "../Contexts/auth-context";
import axios from "axios";

const getImageType = (image) => {
    if (image.startsWith("/9j/")) {
      return "image/jpeg";
    } else if (image.startsWith("iVBORw0KGgo")) {
      return "image/png";
    } else if (image.startsWith("PHN2Zy")) {
      return "image/svg+xml";
    } else if (image.startsWith("R0lGODlh")) {
      return "image/gif";
    } else {
      return "";
    }
};

const Profileinfo = () => {
    const authCtx = useContext(AuthContext);
    const navigate = useNavigate();

    const [firstNameIsValid, setFirstNameIsValid] = useState(true);
    const [lastNameIsValid, setLastNameIsValid] = useState(true);
    const [addressIsValid, setAddressIsValid] = useState(true);
    const [dateIsValid, setDateIsValid] = useState(true);

    const [formIsValid, setFormIsValid] = useState(true);

    const firstNameInputRef = useRef();
    const lastNameInputRef = useRef();
    const addressInputRef = useRef();
    const dateInputRef = useRef();

    const birthDate = new Date(authCtx.user.BirthDate);
    const day = String(birthDate.getDate()).padStart(2, "0");
    const month = String(birthDate.getMonth() + 1).padStart(2, "0");
    const year = birthDate.getFullYear();
    const formatDate = `${year}-${month}-${day}`;
    let role = "";
    var imageURL = "";
    if (authCtx.user.Avatar != null){
        imageURL = `data:${getImageType(authCtx.user.Avatar)};base64,${authCtx.user.Avatar}`;
    }

    if(authCtx.user.Role === 1) {
        role = "Buyer";
    }
    else if(authCtx.user.Role === 2){
        role = "Seller";
    }
    else{
        role = "Admin";
    }

    const firstNameChangeHandler = (event) => {
        const regex = /^(?=.*[a-zA-Z])[a-zA-Z\s]+$/;
        setFirstNameIsValid(regex.test(event.target.value));
    
        setFormIsValid(
        dateIsValid && addressIsValid && lastNameIsValid && firstNameIsValid
        ); 
    };
    
    const lastNameChangeHandler = (event) => {
        const regex = /^(?=.*[a-zA-Z])[a-zA-Z\s]+$/;
        setLastNameIsValid(regex.test(event.target.value));
    
        setFormIsValid(
        dateIsValid && addressIsValid && lastNameIsValid && firstNameIsValid
        ); 
    };
    
    const addressChangeHandler = (event) => {
        const regex = /^(?=.*[a-zA-Z])[a-zA-Z0-9\s]+$/;
        setAddressIsValid(regex.test(event.target.value));
    
        setFormIsValid(
        dateIsValid && addressIsValid && lastNameIsValid && firstNameIsValid
        ); 
    };

    const dateChangeHandler = (event) => {
        const inputDate = new Date(event.target.value);
        const currentDate = new Date();
        
        // Subtract 18 years from the current date
        const minDate = new Date();
        minDate.setFullYear(currentDate.getFullYear() - 18);
        
        const isDateValid = inputDate <= minDate;
      
        setDateIsValid(isDateValid);
      
        setFormIsValid(
          isDateValid && addressIsValid && lastNameIsValid && firstNameIsValid
        );
    };

    const submitHandler = async (event) => {
        event.preventDefault();
        const formData = new FormData();
        if(formIsValid){
            const button = document.getElementById("save");
            button.textContent = "Saving";
            button.disabled = "true";
            formData.append("UserName", authCtx.user.UserName);
            formData.append("Email", authCtx.user.Email);
            formData.append("FirstName", event.target.firstname.value);
            formData.append("LastName", event.target.lastname.value);
            formData.append("BirthDate", event.target.birthDate.value);
            formData.append("Address", event.target.address.value);
            if(event.target.avatar.files.length > 0){
                formData.append("file", event.target.avatar.files[0]);
            }

            try{
                const response = await axios.post(
                    process.env.REACT_APP_SERVER_URL + "users/updateUser", formData, {headers: {"Content-Type" : "multipart/form-data"}}
                );

                if(response.status === 200){
                    authCtx.user.FirstName=response.data.firstName;
                    authCtx.user.Address=response.data.address;
                    authCtx.user.LastName=response.data.lastName;
                    authCtx.user.BirthDate=response.data.birthDate;
                    authCtx.user.Avatar=response.data.avatar;
                    authCtx.onLogin(authCtx.user);
                    button.textContent = "Save changes";
                    button.disabled = false;
                    navigate('/profileinfo');
                }
            }
            catch(error){
                alert(error.response.data.detail);
                button.textContent = "Save changes";
                button.disabled = false;
            }
        }
        else if(!firstNameIsValid){
            firstNameInputRef.current.focus();
        }
        else if(!lastNameIsValid){
            lastNameInputRef.current.focus();
        }
        else if(!addressIsValid){
            addressInputRef.current.focus();
        }
        else if(!dateIsValid){
            dateInputRef.current.focus();
        }
    };

    return(
        <Fragment>
            <form className={classes.summary} onSubmit={submitHandler}>
                <h2>Profile infos</h2>
                <img className={classes.profilePic} src={imageURL} alt="Profile picture"/>
                <br/>
                <label>Change profile picture</label>
                <br/>
                <input className={classes.input} id="avatar" type="file"/>
                <p>
                    UserName : <b>{authCtx.user.UserName}</b>
                </p>
                <br/>
                <p>
                    First name : <input ref={firstNameInputRef} id="firstname" type="text" className={classes.input} defaultValue={authCtx.user.FirstName} onChange={firstNameChangeHandler}/>
                </p>
                <br/>
                <p>
                    Last name : <input ref={lastNameInputRef} id="lastname" type="text" className={classes.input} defaultValue={authCtx.user.LastName} onChange={lastNameChangeHandler}/>
                </p>
                <br/>
                <p>
                    Email: <b>{authCtx.user.Email}</b>
                </p>
                <p>
                    Address : <input ref={addressInputRef} id="address" type="text" className={classes.input} defaultValue={authCtx.user.Address} onChange={addressChangeHandler}/>
                </p>
                <p>
                    Birth date: : <input ref={dateInputRef} id="birthDate" type="date" className={classes.input} defaultValue={formatDate} onChange={dateChangeHandler}/>
                </p>
                <p>
                    Account type : <b>{role}</b>
                </p>
                {authCtx.user.IsVerified ? (
                    <p>
                        Account status: <b>Verified</b>
                    </p>
                ): (
                    <p>
                        Account status: <b>Pending</b>
                    </p>
                )}
                <center>
                    <Button type="submit" id="save">Save changes</Button>
                </center>
            </form>
        </Fragment>
    );
};

export default Profileinfo;