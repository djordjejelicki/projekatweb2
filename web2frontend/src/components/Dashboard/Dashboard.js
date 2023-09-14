import React, { Fragment, useEffect } from "react";
import classes from './Dashboard.module.css';

const Dashboard = () => {
   
    return(
        <Fragment>
            <section className={classes.summary}>
                <h2>items availbale at store</h2>
                <h2>no availbale items at the moment</h2>
            </section>
        </Fragment>
    );
};

export default Dashboard;