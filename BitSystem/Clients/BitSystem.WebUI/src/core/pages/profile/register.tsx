import { faEnvelope, faLock, faUser } from "@fortawesome/free-solid-svg-icons";
import { Card, Form, Input, isEmail, isRequired, IValidation, useModel, asModelField } from "../../../framework";
import { Link, useNavigate } from "react-router-dom";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faFacebook, faTwitter } from "@fortawesome/free-brands-svg-icons";
import { useEffect, useRef } from "react";

export const validPassword: IValidation = {
    test: (value) => value.match(/^.{3,15}$/),
    message: "Entre uma senha de 3 a 15 letras"
}

export function Register() {

    const navigate = useNavigate();

    const [ model, getValue, validate ] = useModel({
        nickName: '',
        fullName: '',
        email: '',
        password: '',
        confirmPassword: ''
    });

    const doRegister = async () => {

        if (!validate()) {
            return;
        }

        //const value = getValue();
        // const user = {
        //     name: value.user,
        //     nickName: value.password
        // };
        // console.log(user);

        navigate("/dashboard", { replace: true });
    }

    const confirmPassword = useRef({
        test: (value) => {
            const passwordModel = asModelField(model.password);
            return value == passwordModel.valueWatch.value;
        },
        message: "Confirme a senha corretamente"
    } as IValidation).current;

    useEffect(() => {
        const passwordModel = asModelField(model.password);
        const confirmPasswordModel = asModelField(model.confirmPassword);
        passwordModel.valueWatch.on(() => confirmPasswordModel.validate());
    }, []);

    return(
        <section className="hero is-fullheight">
            <div className="hero-body">
                <div className="container">
                    <div className="columns is-centered">
                        <div className="column is-half">
                            <Card>
                                <Form>
                                    <Input icon={faUser} label="Apelido" value={model.nickName} autoFocus
                                        validation={[isRequired]}/>
                                    <Input icon={faUser} label="Nome completo" value={model.fullName}
                                        validation={[isRequired]}/>
                                    <Input icon={faEnvelope} label="Endereço de Email" value={model.email}
                                        validation={[isRequired, isEmail]}/>
                                    <Input icon={faLock} label="Senha" value={model.password} type="password"
                                        validation={[isRequired, validPassword]}/>
                                    <Input icon={faLock} label="Confirma a senha" value={model.confirmPassword} type="password"
                                        validation={[confirmPassword]}/>
                                </Form>

                                <div className="columns is-centered pt-4">
                                    <div className="column is-half">
                                        <button className="button is-primary is-fullwidth" onClick={doRegister}>Registrar</button>
                                    </div>
                                </div>

                                <div className="div-line"></div>

                                <span style={{ fontSize: "2rem", display: "flex", alignContent: "center", justifyContent: "center" }}>
                                    <FontAwesomeIcon style={{ margin: "0 10px" }} icon={faFacebook} color="#3b5998" />
                                    <FontAwesomeIcon style={{ margin: "0 10px" }} icon={faTwitter} color="#1DA1F2" />
                                </span>
                                
                                <div className="text-center">
                                    <p>Já tem conta? <Link to="/login" className="text-primary">Entre com sua conta</Link></p>
                                </div>
                            </Card>
                        </div>
                    </div>
                </div>
            </div>
        </section>
    )
}