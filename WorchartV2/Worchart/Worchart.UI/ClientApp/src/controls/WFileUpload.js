import React, { useState, useEffect, useRef } from 'react';
import RS from '../service/LanguageMapService';

function WFileUpload(props) {
    var fileSelector = useRef(null);
    var [files, setFiles] = useState([]);
    var [previewSrc, setPreviewSrc] = useState(props.src);
    var [selectedName, setSelectedName] = useState(RS.get("LBL.SELECTFILE"));

    useEffect(() => {

    }, []);

    function handleFileSelect(e) {
        //e.preventDefault();
        fileSelector.current.click();
    };

    function onChange(e) {
        var fileList = [];
        for (var i in e.target.files) {
            if ((typeof e.target.files[i]) === "object") {
                fileList.push(e.target.files[i]);
            }
        }

        if (fileList && fileList.length > 0) {
            var imageFile = fileList[0];
            setSelectedName(imageFile.name);
            var reader = new FileReader();
            reader.onload = (x) => {
                setPreviewSrc(x.target.result);
            };
            reader.readAsDataURL(imageFile);
        }
        else {
            setSelectedName(RS.get("LBL.SELECTFILE"));
            setPreviewSrc("");
        }

        setFiles(fileList);
        return false;
    }

    function fileUpload(file) {
        //const url = 'http://example.com/file-upload';
        //const formData = new FormData();
        //formData.append('file', file)
        //const config = {
        //    headers: {
        //        'content-type': 'multipart/form-data'
        //    }
        //}
        //return post(url, formData, config)
    }

    return (
        <div className={"w-file-upload " + props.className}>
            <div className={"preview " + (previewSrc ? "" : "dn")}>
                <img src={previewSrc} />
            </div>
            <div className="selection" onClick={handleFileSelect}>
                <span>{selectedName}</span>
                <input type="file" name={props.id} id={props.id} ref={fileSelector} onChange={onChange} className="dn" />
                <a className="btn btn-success btn-pick-file" href="javascript:void(0)">{RS.get("BTN.SELECTFILE")}</a>
            </div>
        </div>
    );
}

export default WFileUpload;