import { useState } from "react";
import loginService from "./login-service";
import { useNavigate } from "react-router-dom";

function Logup() {
    const [userName, setUserName] = useState("");
    const [password, setPassword] = useState("");
    const navigate = useNavigate();

    function logup(e) {
        e.preventDefault()
        let result = loginService.logup(userName, password);
        navigate('/tasks')
    }

    return (
        <section className="main" style={{ display: "block" }}>
            <header className="header">
                <h1>Logup</h1>
                <form onSubmit={logup}>
                    <input className="new-todo" placeholder="userName" value={userName} onInput={(e) => setUserName(e.target.value)} />
                    <input className="new-todo" placeholder="password" value={password} onInput={(e) => setPassword(e.target.value)} />
                    <input type="submit"/>
                </form>
            </header>
        </section>
    )
}

export default Logup;