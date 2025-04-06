

export interface IValidation {
    test: (value: any) => boolean;
    message?: string;
}

// export interface IValidator {
//     validate: (value: any) => [boolean, string[] | null];
// }

// export const useValidate = (validations: IValidation[]): IValidator => {

//     const validate = (value: any): [boolean, string[] | null] => {

//         var messages = validations
//             .filter(v => v.test(value))
//             .map(v => v.message ?? "Validation fail");
//         return [
//             messages.length == 0,
//             messages.length == 0 ? null : messages
//         ]
//     }

//     return {
//         validate
//     }
// }

export const isRequired: IValidation = {
    test: (value) => !!value,
    message: "Campo obrigatório"
}

export const isEmail: IValidation = {
    test: (value) => value
        .toLowerCase()
        .match(/^(([^<>()[\]\\.,;:\s@"]+(\.[^<>()[\]\\.,;:\s@"]+)*)|.(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/),
    message: "Email invalid"
}