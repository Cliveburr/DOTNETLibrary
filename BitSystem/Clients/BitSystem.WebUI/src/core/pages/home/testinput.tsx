// import { useState } from "react";

// const App = () => {
//     const [fields, setFields] = useState({});
//     const [errors, setErrors] = useState({});
    
//     const handleValidation = () => {
//       const formFields = {...fields};
//       const formErrors = {};
//       let formIsValid = true;
  
//       //Name
//       if(!formFields["name"]){
//         formIsValid = false;
//         formErrors["name"] = "Cannot be empty";
//       }
  
//       if(typeof formFields["name"] !== "undefined"){
//         if(!formFields["name"].match(/^[a-zA-Z]+$/)){
//           formIsValid = false;
//           formErrors["name"] = "Only letters";
//         }       
//       }
  
//       //Email
//       if(!formFields["email"]){
//         formIsValid = false;
//         formErrors["email"] = "Cannot be empty";
//       }
  
//       if(typeof formFields["email"] !== "undefined"){
//         let lastAtPos = formFields["email"].lastIndexOf('@');
//         let lastDotPos = formFields["email"].lastIndexOf('.');
  
//         if (!(lastAtPos < lastDotPos && lastAtPos > 0 && formFields["email"].indexOf('@@') == -1 && lastDotPos > 2 && (fields["email"].length - lastDotPos) > 2)) {
//           formIsValid = false;
//           formFields["email"] = "Email is not valid";
//         }
//       }     

//       setErrors(formErrors)
//       return formIsValid;
//     }
    
//     const handleChange = (field, value) => {
//       setFields({
//         ...fields,
//         [field]: value
//       })
//     }
    
//     const contactSubmit = (e) => {
//       e.preventDefault();
//       if(handleValidation()){
//         alert("Form submitted");
//       }else{
//         alert("Form has errors.")
//       }
//     }
    
//     return (
//       <form onSubmit= {e => contactSubmit(e)}>
//         <div>
//           <input type="text" placeholder="Name" onChange={e => handleChange('name', e.target.value)} value={fields["name"]}/>
//           <span className="error">{errors["name"]}</span>
//         </div>
        
//         <div>
//           <input type="text" placeholder="Email" onChange={e => handleChange('email', e.target.value)} value={fields["email"]}/>
//           <span className="error">{errors["email"]}</span>
//         </div>
      
//         <button className="btn btn-lg pro" id="submit" value="Submit">Send Message</button>
//       </form>
//     )
// };

// export default App;