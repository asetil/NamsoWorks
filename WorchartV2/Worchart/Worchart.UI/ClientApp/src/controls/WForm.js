import React, { useState } from 'react';
import RS from '../service/LanguageMapService';
import Winput from './Winput';

function WForm(props) {
    var [model] = useState(props.form.model);
    var [error, setError] = useState(props.form.error);
    var [success, setSuccess] = useState(props.form.success);
    var [title, setTitle] = useState(props.title);
    var [items, setItems] = useState(props.form.items || []);

    function getFields() {
        var fields = items.map(i => {
            i.ref = React.createRef();
            var id = props.form.id + "_" + i.field;
            var value = model ? model[i.field] : "";
            return <Winput key={id} type={i.type} title={i.title} ref={i.ref} value={value} id={id} message={i.message || "MSG.INPUTREQUIRED"} maxLength={i.maxLength} />;
        });
        return fields;
    }

    function onSubmit(e) {
        if (validate() && props.onSubmit) {
            for (var i in items) {
                var item = items[i];
                model[item.field] = item.ref.current.getValue();
            }

            var error = props.onSubmit(model);
            if (error) {
                setError(error);
            }
        }
        return false;
    }

    function validate() {
        var valid = true;
        items.forEach(i => {
            if (i.required && !i.ref.current.isValid()) {
                valid = false;
            }
        });
        return valid;
    }

    return (
        <form action={props.action} method="post" role="form" className={"worchart-form " + props.className} autoComplete="off" noValidate>
            <h3>{RS.get(title)}</h3>
            {error && <p className="text-center error-text"><i className="fa fa-exclamation-mark"></i> {RS.get(error)}</p>}
            {success && <p className="text-center success-text"><i className="fa fa-check"></i> {RS.get(success)}</p>}

            <div className="row">
                {getFields()}

                <div className="form-group col-md-6 mt20">
                    <button type="button" className="w100 btn btn-danger" onClick={props.onCancel}>{RS.get("BTN.CANCEL")}</button>
                </div>

                <div className="form-group col-md-6 mt20">
                    <button type="button" className="w100 btn btn-success" onClick={() => { return onSubmit(); }}>{RS.get("BTN.SAVE")}</button>
                </div>
            </div>
        </form>
    );
}

export default WForm;