//import { PortraitModel } from "@ten/framework_interface";

export interface LoginAuthenticationRequest {
    login: string;
    password: string;
}

export interface TokenAuthenticationRequest {
    token: string;
}

export interface AuthenticationResponse {
    token: string;
    profile: {
        name: string;
        nickName: string;
        //portrait: PortraitModel;
    }
}

// export interface IProfileViewModel {
//     _id: string;
//     nickName: string;
//     name: string;
//     email: string;
//     portrait: PortraitModel;
// }
