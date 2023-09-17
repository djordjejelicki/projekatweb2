import React, { Fragment, useContext } from "react";
import classes from "./NewItem.module.css";
import { useNavigate } from "react-router-dom";
import AuthContext from "../../Contexts/auth-context";
import Button from "../../components/UI/Button/Button";
import axios from "axios";

const NewItem = () => {
    const navigate = useNavigate();
    const ctx = useContext(AuthContext);

    const SubmitHandler = async(event) => {

        event.preventDefault();

        const formData = new FormData();
        formData.append('Name', event.target.name.value);
        formData.append('Description', event.target.description.value);
        formData.append('Price', event.target.price.value);
        formData.append('Amount', event.target.amount.value);
        formData.append('UserId', ctx.user.Id);
        if(event.target.picture.files.length > 0) {
            formData.append('file', event.target.picture.files[0]);
        }

        try{
            const response = await axios.post(process.env.REACT_APP_SERVER_URL + 'items/addNew', formData, 
            {
                headers : {
                    'Content-Type' : 'multipart/form-data',
                    Authorization : `Bearer ${ctx.user.Token}`
                }
            });
            alert(response.data);
            navigate("/");
        }
        catch(error){
            console.error(error);
            alert(error);
        }

    };

    return(
        <Fragment>
            <section className={classes.newItem}>
                <h2>Add new item</h2>
                <form onSubmit={SubmitHandler}>
                    <div className={classes.control}>
                        <label>Name:</label>
                        <input type="text" name="name"/>
                    </div>
                    <div className={classes.control}>
                        <label>Description:</label>
                        <input type="text" name="description"/>
                    </div>
                    <div className={classes.control}>
                        <label>Price:</label>
                        <input type="number" step='0.01' name="price"/>
                    </div>
                    <div className={classes.control}>
                        <label>Amount</label>
                        <input type="number" name="amount"/>
                    </div>
                    <div className={classes.control}>
                        <label>Upload picture:</label>
                        <input type="file" name="picture"/>
                    </div>
                    <div className={classes.control}>
                        <Button type="submit">Add</Button>
                    </div>
                </form>
            </section>
        </Fragment>
    );
};

export default NewItem;