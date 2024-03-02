import { useState } from "react";
import loginService from "./login-service";
import { useNavigate } from "react-router-dom";

function Login() {
    const [userName, setUserName] = useState("");
    const [password, setPassword] = useState("");
    const navigate = useNavigate();

    function login(e) {
        e.preventDefault()
        let result = loginService.login(userName, password);
        navigate('/tasks')
    }

    return (
        <section className="main" style={{ display: "block" }}>
            <header className="header">
                <h1>Login</h1>
                <form onSubmit={login}>
                    <input className="new-todo" placeholder="userName" value={userName} onInput={(e) => setUserName(e.target.value)} />
                    <input className="new-todo" placeholder="password" value={password} onInput={(e) => setPassword(e.target.value)} />
                    <input type="submit"/>
                </form>
            </header>
        </section>
    )
}

export default Login;