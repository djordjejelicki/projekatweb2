import React,{useState,useEffect} from "react";
import User from "../Models/User";

const AuthContext = React.createContext({
    isLoggedIn:false,
    onLogout: ()=>{},
    onLogin:(user)=>{},
});

export const AuthContextProvider = props => {
    const [isLoggedIn, setIsLoggedIn] = useState(false);
    const [user, setUser] = useState(JSON.parse(localStorage.getItem('user')));

    useEffect(()=>{
        const storedUserLoggedInInformation = localStorage.getItem('isLoggedIn');

        if(storedUserLoggedInInformation === '1'){
            setIsLoggedIn(true)
            setUser(JSON.parse(localStorage.getItem('user')));
        }
        else{
            setIsLoggedIn(false);
            setUser(new User(0,'','','','','','','',0,false,''));
        }
    }, []);

    const logoutHandler = () => {
        localStorage.removeItem('isLoggedIn');
        localStorage.removeItem('user');
        setIsLoggedIn(false);
        setUser(new User(0,'','','','','','','',0,false,''));
    }

    const loginHandler = (user) => {
        localStorage.setItem('user', JSON.stringify(user));
        localStorage.setItem('isLoggedIn','1');
        setIsLoggedIn(true);
        setUser(user);
    }

    return (
        <AuthContext.Provider value={{
            isLoggedIn: isLoggedIn, 
            user: user,
            onLogout: logoutHandler,
            onLogin: loginHandler,
            }}>
            {props.children}
        </AuthContext.Provider>
    );

}

export default AuthContext;