Official SHA-256 checksums are provided in the release assets.

See `SHA256SUMS.txt` for details.

---

# Seisei Center v1.1.03

A Windows .NET (C# / WinForms) tool for generating randomized address/value pairs  
with flexible, template-based output.

This tool is intended for use cases such as cheat code creation, memory analysis,
or any situation where large amounts of structured values need to be generated.

---

## Features

- Random generation from **one or more address ranges**
- Configurable value and compare-value ranges
- **Template-based free-format output**
- Normal Mode / Game Genie Mode
- Save and load settings
- Copy generated results to clipboard

---

## Template System

The output format can be freely customized using templates.

### Available placeholders

- `{ADDR}` – Address (hexadecimal)
- `{VAL}`  – Value (hexadecimal)
- `{CMP}`  – Compare value (hexadecimal)

### Examples

```text
{ADDR}:{VAL}
````

```text
if ({ADDR} == {CMP}) then write {VAL}
```

Templates are available only in **Normal Mode**.
They are disabled when **Game Genie Mode** is enabled.

---

## Game Genie Mode

Supports NES Game Genie code generation.

* 6-character and 8-character codes
* Bit conversion and shuffling implemented according to known specifications

Template output is not used in Game Genie Mode.

---

## Antivirus False Positives

This application may be flagged as suspicious or as a PUA (Potentially Unwanted Application)
by some antivirus software.

This is caused by characteristics such as:

* Being an unsigned, self-built .NET executable
* Use of random value generation
* Template-based code output behavior

This application **does not** perform any of the following actions:

* Network communication
* Persistence or auto-start registration
* Privilege escalation
* Code injection or obfuscation

The full source code is publicly available in this repository.

---

## Inspiration

The idea for this tool was inspired by the following website:

[https://seiseicenter.lutica.net/](https://seiseicenter.lutica.net/)

This project is **not affiliated** with the above site.

---

## License

This project is licensed under the Apache License, Version 2.0.

See the [LICENSE](LICENSE) file for details.


---

# 生成センター v1.1.03

アドレス・値・比較値をもとに、
ランダム生成およびテンプレート出力を行う
Windows 向け .NET（C# / WinForms）ツールです。

チートコード作成やメモリ解析など、
「一定の形式で大量の値を生成したい」用途を想定しています。

---

## 主な機能

* **複数指定可能なアドレス範囲**からのランダム生成
* 値・比較値の範囲指定
* **テンプレートによる自由な出力形式**
* 通常モード / Game Genie モード切替
* 設定の保存・読み込み
* 生成結果のコピー機能

---

## テンプレート機能

テンプレートを使って出力形式を自由に指定できます。

### 使用可能なプレースホルダ

* `{ADDR}` … アドレス（16進数）
* `{VAL}`  … 値（16進数）
* `{CMP}`  … 比較値（16進数）

### 使用例

```text
{ADDR}:{VAL}
```

```text
if ({ADDR} == {CMP}) then write {VAL}
```

テンプレートは **通常モード時のみ** 使用可能です。
Game Genie モードでは無効になります。

---

## Game Genie モード

NES 用 Game Genie コードの生成に対応しています。

* 6文字 / 8文字コード対応
* 仕様に基づいたビット変換およびシャッフル処理

Game Genie モードではテンプレート出力は使用されません。

---

## ウイルス対策ソフトの誤検知について

本ツールは以下の理由により、一部のウイルス対策ソフトで
**誤検知（Suspicious / PUA）** される場合があります。

* 署名のない自作 .NET 実行ファイルであること
* ランダム値生成を行うこと
* テンプレートによるコード生成挙動を持つこと

本ツールは以下の動作を **一切行いません**：

* ネットワーク通信
* 常駐化・自動起動登録
* 権限昇格
* コードインジェクションや難読化処理

ソースコードは本リポジトリにてすべて公開しています。

---

## 発想の元

本ツールの発想の元になったサイト：

[https://seiseicenter.lutica.net/](https://seiseicenter.lutica.net/)

本プロジェクトは上記サイトとは**無関係**です。

---

## ライセンス

本プロジェクトは Apache License 2.0 のもとで公開されています。

詳細は [LICENSE](LICENSE) ファイルを参照してください。

