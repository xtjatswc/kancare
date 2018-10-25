
<?php

if(isset($_POST["btnSubmit"])){
    $a = $_POST["txtA"];

    $submit = $_POST["btnSubmit"];
    if($submit == "base64_encode >>"){
        $b = $_POST["txtB"];
        $a = base64_encode($b);
    }else if($submit == "<< base64_decode"){
        $a = $_POST["txtA"];
        $b = base64_decode($a);
    }
}
?>
<!DOCTYPE html> 
<html>
    <head>
        <meta charset="utf-8" />
        <meta http-equiv="X-UA-Compatible" content="IE=edge">
        <title>base64转换</title>
        <meta name="viewport" content="width=device-width, initial-scale=1">
        <style>
        input[type="button"],input[type="submit"]{width:150px;height:50px;font-size: 12pt;margin:3px;}
        textarea{font-size: 15pt;color:blue;}
        </style>
        <script>
        function o(id){
            return document.getElementById(id);
        }

        function convertBtoA(){
            o("txtA").value = btoa(o("txtB").value);
        }

        function convertAtoB(){
            try {
                o("txtB").value = atob(o("txtA").value);
            } catch (error) {
                alert(error);
            }
        }

        function clearA(){
            o("txtA").value="";
        }

        function clearB(){
            o("txtB").value="";
        }
        </script>
    </head>
    <body>
        <form action="" method="post">
            <table>
                <tr>
                    <td><textarea id="txtB" name="txtB" rows="30" cols="60"><?php if(isset($b)){echo $b;}?></textarea></td>
                    <td>
                        <input id="btnClearA" type="button" value="clear >>" onclick="clearA()"><br/>
                        <input id="btnClearB" type="button" value="<< clear" onclick="clearB()"><br/>
                        <input id="btnBtoa" type="button" value="btoa >>" onclick="convertBtoA()"><br/>
                        <input id="btnAtob" type="button" value="<< atob" onclick="convertAtoB()"><br/>
                        <input name="btnSubmit" type="submit" value="base64_encode >>"><br/>
                        <input name="btnSubmit" type="submit" value="<< base64_decode"><br/>                        
                    </td>
                    <td><textarea id="txtA" name="txtA" rows="30" cols="60"><?php if(isset($a)){echo $a;}?></textarea></td>
                </tr>
            </table>
        </form>
    </body>
</html>
