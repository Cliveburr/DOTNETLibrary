import { IconDefinition } from "@fortawesome/fontawesome-svg-core";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { useEffect, useRef, useState } from "react";
import { classNames, } from "../../helpers";
import { FieldState, isModelField, useModelField } from "./useModel";
import { IValidation } from "./validation";
import { faCheck, faXmark } from '@fortawesome/free-solid-svg-icons';

interface FieldProps {
    type?: "text" | "password" | "email";
    value?: string;
    label?: string;
    name?: string;
    placeholder?: string;
    onChange?: (e: React.ChangeEvent<HTMLInputElement>) => void;
    autoFocus?: boolean;
    icon?: IconDefinition;
    onEnter?: () => void;
    validation?: Array<IValidation>;
}

// enum FieldStatus {
//     Init,
//     Pristine,
//     Valid,
//     Invalid
// }

export const Input = (args: FieldProps) => {

    //const [status, setStatus] = useState(args.validation ? FieldStatus.Init : FieldStatus.Pristine);
    //const [messages, setMessages] = useState<Array<string> | null>(null);
    const model = useRef(isModelField(args.value) ? args.value : useModelField(args.value, '')).current;
    const [value, setValue] = useState(model.valueWatch.value);
    const [state, setState] = useState(model.stateWatch.value);
    // const [value, setValue] = isModelField(args.value) ?
    //     [args.value.getValue(), args.value.setValue] : useState(args.value || "");
    //const [value, setValue] = useState(isModelField(args.value) ? args.value.getValue() : args.value || "");
    
    const type = args.type || "text";
    const placeholder = args.placeholder || args.label;
    
    const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        //setValue(e.target.value);
        model.valueWatch.value = e.target.value;

        // if (isModelField(args.value)) {
        //     args.value.setValue(e.target.value);
        // }
    };

    const handleKeyDown = (e: React.KeyboardEvent<HTMLInputElement>) => {
        if (e.key == "Enter" && args.onEnter) {
            args.onEnter();
        }
    }

    useEffect(() => {
        if (args.validation) {
            model.validatorsWatch.update(v => v.push(...args.validation!), false);
            // const validator = useValidate(args.validation);

            // useEffect(() => {
            //     if (status == FieldStatus.Init) {
            //         setStatus(FieldStatus.Pristine);
            //     }
            //     else {
            //         const [isValid, errorMessages] = validator.validate(value);
            //         setStatus(isValid ? FieldStatus.Valid : FieldStatus.Invalid)
            //         setMessages(errorMessages);
            //     }
            // }, [value]);
        }

        model.valueWatch.on(() => setValue(model.valueWatch.value));
        model.stateWatch.on(() => setState(model.stateWatch.value));
        //useEffect(() => {}, [modelField.getValue(), modelField.getState(), modelField.getMessages()]);
    }, []);

    return (
        <div className="field">
            {args.label ?? (<label className="label">{args.label}</label>)}
            <div className={classNames("control", args.icon ? "has-icons-left" : null, state != FieldState.Pristine ? "has-icons-right": null)}>
                <input
                    className={classNames("input",
                        state == FieldState.Valid ? "is-success": null,
                        state == FieldState.Invalid ? "is-danger": null)}
                    type={type}
                    placeholder={placeholder}
                    value={value}
                    onChange={handleChange}
                    onKeyDown={handleKeyDown}
                    autoFocus={args.autoFocus} />
                {args.icon ? (<span className="icon is-small is-left">
                    <FontAwesomeIcon icon={args.icon} />
                </span>): null}
                {state == FieldState.Valid ? <span className="icon is-small is-right has-text-success">
                    <FontAwesomeIcon icon={faCheck} />
                </span> : null}
                {state == FieldState.Invalid ? <span className="icon is-small is-right has-text-danger">
                    <FontAwesomeIcon icon={faXmark} />
                </span> : null}
            </div>
            {model.messagesWatch.value.map((m, i) => <p key={i} className="help is-danger">{m}</p>)}
        </div>
    )
}