import { watch, WatchValue } from "../../helpers";
import { IValidation } from "./validation";

// export interface IModel {
//     fields: IModelField[];
// }

// export interface IModelObject {

// }

export enum FieldState {
    Pristine,
    Valid,
    Invalid
}

export interface IModelField {
    name: string;
    valueWatch: WatchValue<any>;
    validatorsWatch: WatchValue<IValidation[]>;
    stateWatch: WatchValue<FieldState>;
    messagesWatch: WatchValue<string[]>;
    validate: () => [boolean, string[]];
}

export const useModel = <T,>(model: T): [T, () => T, () => boolean] => {

    const fieldsName = Object.getOwnPropertyNames(model)
        .map(n => useModelField((<any>model)[n], n));

    const modelValue = {} as any;
    fieldsName.forEach(f => {
        modelValue[f.name] = f;
    });
    
    const getValue = (): T => {
        const value = {} as any;
        fieldsName.forEach(f => {
            value[f.name] = f.valueWatch.value;
        });
        return value;
    }

    const validate = (): boolean => {
        fieldsName.forEach(f => {
            f.validate();
        });
        const valid = fieldsName
            .every(f => f.stateWatch.value == FieldState.Valid);
        return valid;
    }

    return [
        modelValue,
        getValue,
        validate
    ];
}

export const useModelField = (pValue: any, name: string): IModelField => {
    
    const valueWatch = watch(pValue);
    const validatorsWatch = watch<IValidation[]>([]);
    const stateWatch = watch(FieldState.Pristine);
    const messagesWatch = watch<Array<string>>([]);

    const validate = (): [boolean, string[]] => {

        const errorMessages = validatorsWatch.value
            .filter(v => !v.test(valueWatch.value))
            .map(v => v.message ?? "Validation fail");

            stateWatch.value = (errorMessages.length == 0 ? FieldState.Valid : FieldState.Invalid)
            messagesWatch.value = errorMessages;

        return [
            errorMessages.length == 0,
            errorMessages
        ]
    }

    valueWatch.on(validate);
    validatorsWatch.on(validate);
    // useEffect(() => {
    //     validate();
    // }, [value, validators]);

    return {
        name,
        valueWatch,
        validatorsWatch,
        stateWatch,
        messagesWatch,
        validate
    }
}

export const isModelField = (value: any): value is IModelField => {
    return value.hasOwnProperty("valueWatch") && value.hasOwnProperty("validatorsWatch") &&
        value.hasOwnProperty("stateWatch") && value.hasOwnProperty("messagesWatch") &&
        value.hasOwnProperty("name") && value.hasOwnProperty("validate");
}

export const asModelField = (value: any): IModelField => {
    if (isModelField(value)) {
        return value;
    }
    throw 'Invalid value as IModelField';
}