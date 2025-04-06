import { useContext } from "react";
import { Link, useNavigate } from "react-router-dom";
import { AuthContext, Card, Input, Form, useModel, isRequired, loginAuthentication } from "../../../framework";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faUser, faLock } from '@fortawesome/free-solid-svg-icons';
import { faFacebook, faTwitter } from '@fortawesome/free-brands-svg-icons';

export function Login() {

    const authContext = useContext(AuthContext)!;
    const navigate = useNavigate();

    const [ model, getValue, validate ] = useModel({
        user: '',
        password: ''
    });

    const doLogin = async () => {

        if (!validate()) {
            return;
        }

        const value = getValue();
        const loginResponse = await loginAuthentication({
            login: value.user,
            password: value.password
        })
        
        console.log(loginResponse);

        authContext.setAuth(loginResponse);
        navigate("/dashboard", { replace: true });
    }

    return(
        <section className="hero is-fullheight">
            <div className="hero-body">
                <div className="container">
                    <div className="columns is-centered">
                        <div className="column is-half">
                            <Card>
                                <Form>
                                    <Input icon={faUser} label="Apelido ou Email" value={model.user} autoFocus
                                        validation={[isRequired]}/>
                                    <Input icon={faLock} label="Senha" value={model.password} type="password" onEnter={doLogin}
                                        validation={[isRequired]}/>
                                </Form>

                                <div className="columns is-centered pt-4">
                                    <div className="column is-half">
                                        <button className="button is-primary is-fullwidth" onClick={doLogin}>Entrar</button>
                                    </div>
                                </div>

                                <div className="div-line"></div>

                                <span style={{ fontSize: "2rem", display: "flex", alignContent: "center", justifyContent: "center" }}>
                                    <FontAwesomeIcon style={{ margin: "0 10px" }} icon={faFacebook} color="#3b5998" />
                                    <FontAwesomeIcon style={{ margin: "0 10px" }} icon={faTwitter} color="#1DA1F2" />
                                </span>
                                
                                <div className="text-center">
                                    <p>Não é registrado? <Link to="/register" className="text-primary">Crie sua conta</Link></p>
                                </div>
                            </Card>
                        </div>
                    </div>
                </div>
            </div>
        </section>
    )
}