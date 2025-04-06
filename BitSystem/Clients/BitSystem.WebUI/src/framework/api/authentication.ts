import { LoginAuthenticationRequest } from "../models";
import { IAuthProfile } from "../contexts";

const delay = (count: number) => {
    return new Promise((e) => {
        setTimeout(e, count);
    });
}

export const loginAuthentication = async (model: LoginAuthenticationRequest): Promise<IAuthProfile> => {
    //const response = <any>{}// await this.securityBusiness.authenticationByLogin(model);
    //this.sessionService.startSession(response.token, response.profile);
    //this.user.set({});
    await delay(2000);

    return {
        name: 'test',
        nickName: 'test'
    };
}

export const Authentication = {

    tokenAuthentication: async (): Promise<void> => {
        // const token = this.sessionService.readSessionToken();
        // if (token) {
        //     try {
        //         // const response = await this.securityBusiness.authenticationByToken({
        //         //     token
        //         // });
        //         const response = <any>{}
        //         this.sessionService.startSession(response.token, response.profile);
        //     } 
        //     catch {
        //         this.logoff();
        //     }
        // }
    },

    logoff: async (): Promise<void> => {
        //await this.securityBusiness.logoff();
        //this.sessionService.endSession();
    }
}

